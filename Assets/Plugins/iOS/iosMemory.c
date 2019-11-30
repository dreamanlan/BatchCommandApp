#import <mach/mach.h>

float ios_GetAppMemory(void) {
    task_vm_info_data_t vmInfo;
    mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
    kern_return_t kernelReturn = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t) &vmInfo, &count);
    if( kernelReturn == KERN_SUCCESS ) {
      return vmInfo.phys_footprint/1024.0f/1024.0f;
    } else {
      return 0.0f;
    }
}
